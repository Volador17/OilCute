function [w,t,p,b,press,yreg,yp,ye,yte,md,rms,mdt,nndt,my,cvR,mdc,yecv,R,sec,secv]=plsc(x,y,k)
    
%ƫ��С���˻ع����PLS1��
%w��Ȩ��ʸ����
%t���÷֣�
%p���غɣ�
%b���ع�ϵ����
%press��������֤�в�ƽ���ͣ�
%yreg:Ԥ��ֵ��
%ye���в
%yet:ѧ���в
%md:���Ͼ��룻
%rms�����ײв
%mdt�����Ͼ�����ֵ��
%nndt�����ڽ�������ֵ��
%my��y��ƽ��ֵ��
%R:Ԥ��ֵ��ʵ��ֵ֮������ϵ����
%sec��У����Ԥ���׼ƫ�
%secv��������֤��׼��
%x�����׾���
%y������������
%k���������������



[m,n]=size(x);


%������֤
for i=1:n
    xx=x(:,[1:(i-1), (i+1):n]);
    px=y(:,[1:(i-1), (i+1):n]);
    [w,t,p,b,mmy]=plsc1(xx,px,k);
    ypp=0;
    xy=x(:,i)';
    for j=1:k
        tt(:,j)=xy*w(:,j);
        yp(i,j)=ypp+b(j)*tt(:,j)+mmy;
        ypp=ypp+b(j)*tt(:,j);
        xy=xy-tt(:,j)*p(:,j)';
    end
    tc(i,:)=tt;
end

 %---------------------------------------------------
    tc=tc';
for i=1:k
    press(i)=(y-yp(:,i)')*(y-yp(:,i)')';
    secv(i)=sqrt(press(i)/n);


    %����в�
    yecv(i,:)=yp(:,i)'-y;
 %�������ϵ����
cvR(i)=(sum((yp(:,i)'-mmy).*(yp(:,i)'-mmy)))/(sum(yecv(i,:).*yecv(i,:))+sum((yp(:,i)'-mmy).*(yp(:,i)'-mmy)));
   
    %�������Ͼ���

    mdc(i,:)=mahalr(tc(1:i,:),tc(1:i,:));
 %------------------------------------------------------
end

[w,t,p,b,my]=plsc1(x,y,k);%����ƫ��С���˻ع����
t=t';
p=p';
ypp=0;
x=x';
 for i=1:k
    %����Ԥ��ֵ
    yreg(i,:)=(ypp+b(i)*t(i,:)'+my)';
    ypp=ypp+b(i)*t(i,:)';
    x=x-t(i,:)'*p(i,:);

        %����в�
    ye(i,:)=yreg(i,:)-y;
    sec(i)=sqrt(ye(i,:)*ye(i,:)'/(n-1));%����Ԥ���׼ƫ��

    
    %�������ϵ����
    R(i)=(sum((yreg(i,:)-mmy).*(yreg(i,:)-mmy)))/(sum(ye(i,:).*ye(i,:))+sum((yreg(i,:)-mmy).*(yreg(i,:)-mmy)));



    %�������Ͼ��뼰��ֵ
    md(i,:)=mahalr(t(1:i,:),t(1:i,:));
    mdt(i)=max(md(i,:));

    
    %����ѧ���в�
    yte(i,:)=abs(ye(i,:)./(sec(i)*sqrt(1-md(i,:))));
    
    %�������ڽ�����
    nd(i,:)=nndr(t(1:i,:),t(1:i,:));
    nndt(i)=max(nd(i,:));
    
    %������ײв�
    rms(i,:)=rmssr(x',p(1:i,:));
    end

     yp=yp';