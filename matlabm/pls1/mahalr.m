function md=mahalr(x1,x2);

%���Ͼ���������;
%x1������1;
%x2������2��һ��ΪУ��������
%�������1�е�ÿ���㵽����2�����Ͼ��롣




[rx1,cx1] = size(x1);
[rx2,cx2] = size(x2);

mx2=mean(x2,2);
dx1=x1-mx2(:,ones(cx1,1));
dx2=x2-mx2(:,ones(cx2,1));

ss=inv(dx2*dx2');
for i=1:cx1
    md(i)=dx1(:,i)'*ss*dx1(:,i);
end
