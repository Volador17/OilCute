%����Savitzky-Golay����������
%X������Ⱦ���m�����ڿ�ȣ�����Ϊ�������������3��p����������;����3�ζ���ʽ��
%d��һ��΢�ֺ������Ⱦ���
%��Ҫ�ο��ļ���������ѧ����ѧ����ѧ�����磩��P47
function d=sgdiff(X,m,p);

	k=2;%�������ζ���ʽ

	% ����D����
	s = fliplr(vander(-(m-1)./2:(m-1)./2));
	S = s(:,1:k+1);   
	D1=S'*S;
	D2=inv(D1);
	D=D2*S';

	[yr,yc]=size(X);
	mm=(m+1)/2;
	mmm=(m-1)/2;

	%���˲���
	d=zeros(yr,yc);

	%���㵼������
	pp=1;
	for ii=1:p
		pp=pp*ii;
	end %���������Ľײ�
	pp=pp*ones(1,yc);
	for i=mm:yr-mmm
		xx=X((i-mmm):(i+mmm),:);
		A=D*xx;
		d(i,:)=A(p+1,:).*pp;
	end
end
