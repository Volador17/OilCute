function [w1,b1,w2,b2]=bann1(x,y,trainf,hm,f1,f2,tn,traino,d,k)

%�޼�ؼ���������ѵ������

%x:���׾���
%y:����������
%trainf:ѵ��������ȡֵΪtraingd��traingdm��trainlm�е�һ��
%hm�������ڵ�����
%f1:��һ�㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%f2:�ڶ��㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%tn:ѵ��������
%traino:ѵ��Ŀ��.
%tn:ѵ��������
%traino:ѵ��Ŀ��.
%d:ƽ������
%k��ѭ��ѵ������
x = x';
y = y';



aa=inf;
nn=[];
if strcmpi(trainf,'traingd')
         for jj=1:d   
    for i=1:k
        [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=tbp2(w1,b1,f1,w2,b2,f2,x,y,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        if seca<aa
         aa=seca;
         ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
        end
    end 
  end
elseif strcmpi(trainf,'traingdm')
        for jj=1:d
      for i=1:k
        [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=tbpx2(w1,b1,f1,w2,b2,f2,x,y,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        if seca<aa
         aa=seca;
     ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
        end
      end     
     end
elseif strcmpi(trainf,'trainlm')
    for jj=1:d
       for i=1:k
        [w1,b1,w2,b2]=initff(x,hm,f1,y,f2);
        [w1,b1,w2,b2]=tlm2(w1,b1,f1,w2,b2,f2,x,y,[100 tn traino 0.001]);
        a1=simuff(x,w1,b1,f1,w2,b2,f2);
        seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
        if seca<aa
         aa=seca;
    ww1(jj,:,:)=w1;ww2(jj,:)=w2;bb1(:,jj)=b1;bb2(jj)=b2;
        end
    end 
end
end
w1=ww1;w2=ww2;b1=bb1;b2=bb2;