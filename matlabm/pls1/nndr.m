function nd=nndr(x1,x2);

%���ڽ�����������
%x1������1;
%x2������2��һ��ΪУ��������
%nd������1�е�ÿ���㵽����2�����ڽ����롣


[rx1,cx1] = size(x1);
[rx2,cx2] = size(x2);

ss=inv(x2*x2');
for i=1:cx1
    for j=1:cx2
        d(j)=(x1(:,i)-x2(:,j))'*ss*(x1(:,i)-x2(:,j));
    end
    iii=find(d==0);
    d(iii)=[];
    nd(i)=min(d);
   
end